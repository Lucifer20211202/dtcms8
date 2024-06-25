<template>
	<div class="mainbody">
		<dt-location :data="[{title:'系统功能'},{title:'编辑授权'}]"></dt-location>
		<div class="content-box">
			<dt-form-box ref="editFormRef" v-model="datas.model" :rules="datas.rules" activeName="info">
				<el-tab-pane label="基本信息" name="info">
					<div class="tab-content">
						<el-form-item label="是否启用">
							<el-switch :active-value="0" :inactive-value="1" v-model="datas.model.status"></el-switch>
						</el-form-item>
						<el-form-item prop="siteId" label="所属站点">
							<el-select v-model="datas.model.siteId" placeholder="请选择...">
								<el-option v-for="item in datas.siteList"
									:key="item.id"
									:label="item.title"
									:value="item.id">
								</el-option>
							</el-select>
						</el-form-item>
						<el-form-item prop="provider" label="开放平台">
							<el-radio-group v-model="datas.model.provider">
								<el-radio-button label="qq">QQ互联</el-radio-button>
								<el-radio-button label="wechat">微信</el-radio-button>
							</el-radio-group>
						</el-form-item>
						<el-form-item prop="type" label="应用类型">
							<el-radio-group v-model="datas.model.type">
								<el-radio-button label="web">网页</el-radio-button>
							</el-radio-group>
						</el-form-item>
						<el-form-item label="显示图标">
							<dt-upload-image v-model="datas.model.imgUrl"></dt-upload-image>
						</el-form-item>
						<el-form-item prop="title" label="标题名称">
							<el-input v-model="datas.model.title" placeholder="必填，128字符内"></el-input>
						</el-form-item>
						<el-form-item prop="clientId" label="AppId">
							<el-input v-model="datas.model.clientId" placeholder="必填，开放平台提供的AppId"></el-input>
						</el-form-item>
						<el-form-item prop="clientSecret" label="AppSecret">
							<el-input v-model="datas.model.clientSecret" placeholder="必填，开放平台提供的AppKey"></el-input>
						</el-form-item>
						<el-form-item prop="sortId" label="排序数字">
							<el-input v-model="datas.model.sortId" placeholder="数字越小越排前"></el-input>
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
	import DtUploadImage from '../../../components/upload/DtUploadImage.vue'
	
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
		siteList: [],
		model: {
			id: 0,
			siteId: null,
			provider: null,
			type: null,
			title: null,
			imgUrl: null,
			clientId: null,
			clientSecret: null,
			sortId: 99,
			status: 0,
			addBy: null,
			addTime: null
		},
		rules: {
			siteId: [
				{ required: true, message: '请选择所属站点', trigger: 'change' }
			],
			provider: [
				{ required: true, message: '请选择接入平台', trigger: 'change' }
			],
			type: [
				{ required: true, message: '请选择应用类型', trigger: 'change' }
			],
			title: [
				{ required: true, message: '请输入标题名称', trigger: 'blur' }
			],
			clientId: [
				{ required: true, message: '请输入开放平台提供的AppId', trigger: 'blur' }
			],
			clientSecret: [
				{ required: true, message: '请输入开放平台提供的AppSecret', trigger: 'blur' }
			],
			sortId: [
				{ required: true, message: '请输入排序数字', trigger: 'blur' }
			]
		}
	})
	
	//初始化数据
	const initData = async() => {
		//加载站点列表
		await proxy.$api.request({
			url: '/admin/site/view/0',
			success(res) {
				datas.siteList = res.data
			}
		})
		//修改时赋值
		if (props.id) {
			await proxy.$api.request({
				url: `/admin/site/oauth/${props.id}`,
				loading: true,
				success(res) {
					datas.model = res.data
				}
			})
		}
	}
	//提交表单
	const submitForm = () => {
		//调用组件验证表单
		editFormRef.value.form.validate((valid) => {
			if(valid) {
				//Id大于0则修改，否则添加
				if (datas.model.id > 0) {
					proxy.$api.request({
						method: 'put',
						url: `/admin/site/oauth/${datas.model.id}`,
						data: datas.model,
						successMsg: '修改授权平台已成功',
						beforeSend() {
							datas.loading = true
						},
						success(res) {
							//跳转列表页
							proxy.$common.linkUrl('/setting/oauth/list')
						},
						complete() {
							datas.loading = false
						}
					})
				} else {
					proxy.$api.request({
						method: 'post',
						url: '/admin/site/oauth',
						data: datas.model,
						successMsg: '新增授权平台已成功',
						beforeSend() {
							datas.loading = true
						},
						success(res) {
							//重置表单
							editFormRef.value.form.resetFields()
						},
						complete() {
							datas.loading = false
						}
					})
				}
			}
		})
	}
	
	//运行初始化
	initData()
</script>