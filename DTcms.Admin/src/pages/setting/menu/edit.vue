<template>
	<div class="mainbody">
		<dt-location :data="[{title:'系统功能'},{title:'前台菜单'},{title:'编辑菜单'}]"></dt-location>
		<div class="content-box">
			<dt-form-box ref="editFormRef" v-model="datas.model" :rules="datas.rules" activeName="info">
				<el-tab-pane label="基本信息" name="info">
					<div class="tab-content">
						<el-form-item label="是否显示">
							<el-switch :active-value="0" :inactive-value="1" v-model="datas.model.status"></el-switch>
						</el-form-item>
						<el-form-item label="所属父级">
							<dt-dropdown-select v-model="datas.model.parentId"
								:disabled="datas.model.id"
								:data="datas.listData"
								placeholder="请选择">
							</dt-dropdown-select>
						</el-form-item>
						<el-form-item prop="title" label="显示标题">
							<el-input v-model="datas.model.title" placeholder="必填，128字符内"></el-input>
						</el-form-item>
						<el-form-item label="副标题">
							<el-input v-model="datas.model.subTitle" placeholder="可空，128字符内"></el-input>
						</el-form-item>
						<el-form-item label="图标样式">
							<dt-upload-text v-model="datas.model.iconUrl" placeholder="图标地址或CSS样式名" exts="jpg,jpeg,png,gif,webp" :size="1024" />
						</el-form-item>
						<el-form-item label="链接地址">
							<el-input v-model="datas.model.linkUrl" placeholder="可空，512字符内"></el-input>
						</el-form-item>
						<el-form-item prop="sortId" label="排序数字">
							<el-input v-model="datas.model.sortId" placeholder="数字越小越排前"></el-input>
						</el-form-item>
						<el-form-item label="备注说明">
							<el-input type="textarea" :rows="2" v-model="datas.model.remark" placeholder="可空，500字符内"></el-input>
						</el-form-item>
						<el-form-item label="创建时间" v-if="datas.model.id>0">
							{{datas.model.addBy}} {{datas.model.addTime}}
						</el-form-item>
					</div>
				</el-tab-pane>
			</dt-form-box>
		</div>
		
		<div class="footer-box">
			<div class="footer-btn">
				<el-button type="primary" :loading="datas.loading" @click="submitForm">确认保存</el-button>
				<el-button plain @click="$common.back()">返回上一页</el-button>
			</div>
		</div>
	</div>
</template>

<script setup>
	import { ref,reactive,getCurrentInstance } from "vue"
	import DtFormBox from '../../../components/layout/DtFormBox.vue'
	import DtUploadText from '../../../components/upload/DtUploadText.vue'
	import DtDropdownSelect from '../../../components/control/DtDropdownSelect.vue'
	
	//获取全局属性
	const { proxy } = getCurrentInstance()
	//Ref对象
	const editFormRef = ref(null)
	
	//接收props传值
	const props = defineProps({
		id: 0
	})
	
	//定义页面属性
	const datas = reactive({
		loading: false,
		listData: [],
		model: {
			id: 0,
			parentId: 0,
			title: null,
			subTitle: null,
			iconUrl: null,
			linkUrl: null,
			remark: null,
			sortId: 99,
			status: 0,
			addBy: null,
			addTime: null
		},
		rules: {
			title: [
				{ required: true, message: '请输入导航菜单标题', trigger: 'blur' }
			],
			sortId: [
				{ required: true, message: '请输入排序数字', trigger: 'blur' },
				{ pattern: /^[+]{0,1}(\d+)$/, message: '只能为正整数', trigger: 'blur' }
			]
		}
	})
	
	//初始化数据
	const initData = async() => {
		let parentId = proxy.$route.query.parentId //获取父ID
		//修改时赋值
		if (props.id) {
			await proxy.$api.request({
				url: `/admin/site/menu/${props.id}`,
				loading: true,
				success(res) {
					datas.model = res.data
				}
			})
		} else if (parentId) {
			datas.model.parentId = parseInt(parentId)
		}
		
		//加载全部菜单
		await proxy.$api.request({
			url: '/admin/site/menu',
			loading: true,
			success(res) {
				datas.listData = res.data
			}
		})
	}
	//提交表单
	const submitForm = () => {
		//调用组件验证表单
		editFormRef.value.form.validate((valid) => {
			if(valid) {
				//如果是数组则取最后一个值
				if (Array.isArray(datas.model.parentId)) {
					datas.model.parentId = datas.model.parentId[datas.model.parentId.length - 1]
				}
				//Id大于0则修改，否则添加
				if (datas.model.id > 0) {
					proxy.$api.request({
						method: 'put',
						url: `/admin/site/menu/${datas.model.id}`,
						data: datas.model,
						successMsg: '修改菜单已成功',
						beforeSend() {
							datas.loading = true
						},
						success(res) {
							//跳转加列表页
							proxy.$common.linkUrl('/setting/menu/list')
						},
						complete() {
							datas.loading = false
						}
					})
				} else {
					proxy.$api.request({
						method: 'post',
						url: '/admin/site/menu',
						data: datas.model,
						successMsg: '新增菜单已成功',
						beforeSend() {
							datas.loading = true
						},
						success(res) {
							//重置表单
							editFormRef.value.form.resetFields()
							initData()
						},
						complete() {
							datas.loading = false
						}
					});
				}
			}
		})
	}
	
	//运行初始化
	initData()
</script>