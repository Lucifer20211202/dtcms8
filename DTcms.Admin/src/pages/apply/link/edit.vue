<template>
	<div class="mainbody">
		<dt-location :data="[{title:'应用管理'},{title:'编辑链接'}]"></dt-location>
		<div class="content-box">
			<dt-form-box ref="editFormRef" v-model="datas.model" :rules="datas.rules" activeName="info">
				<el-tab-pane label="基本信息" name="info">
					<div class="tab-content">
						<el-form-item label="是否启用">
							<el-switch :active-value="1" :inactive-value="0" v-model="datas.model.status"></el-switch>
						</el-form-item>
						<el-form-item label="是否推荐">
							<el-switch :active-value="1" :inactive-value="0" v-model="datas.model.isRecom"></el-switch>
						</el-form-item>
						<el-form-item label="网站LOGO">
							<dt-upload-image v-model="datas.model.logoUrl"></dt-upload-image>
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
						<el-form-item prop="title" label="网站标题">
							<el-input v-model="datas.model.title" placeholder="网站显示的标题，128字符内"></el-input>
						</el-form-item>
						<el-form-item prop="siteUrl" label="链接网址">
							<el-input v-model="datas.model.siteUrl" placeholder="必填，交换链接的网址"></el-input>
						</el-form-item>
						<el-form-item prop="telPhone" label="联系电话">
							<el-input v-model="datas.model.telPhone" placeholder="非必填，网站负责人联系方式"></el-input>
						</el-form-item>
						<el-form-item prop="sortId" label="排序数字">
							<el-input v-model="datas.model.sortId" placeholder="数字越小越排前"></el-input>
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
			title: null,
			telPhone: null,
			siteUrl: null,
			logoUrl: null,
			sortId: 99,
			isRecom: 0,
			status: 1,
			addTime: null
		},
		rules: {
			siteId: [
				{ required: true, message: '请选择所属站点', trigger: 'change' }
			],
			title: [
				{ required: true, message: '请输入网站标题', trigger: 'blur' }
			],
			siteUrl: [
				{ required: true, message: '请输入交换链接网址', trigger: 'blur' }
			],
			telPhone: [
				{ required: true, message: '请输入网站负责人电话', trigger: 'blur' }
			],
			sortId: [
				{ required: true, message: '请输入排序数字', trigger: 'blur' }
			]
		},
	})
	
	//初始化数据
	const initData = async() => {
		//修改时赋值
		if (props.id) {
			await proxy.$api.request({
				url: `/admin/link/${props.id}`,
				loading: true,
				success(res) {
					datas.model = res.data
				}
			})
		}
		//加载站点列表
		await proxy.$api.request({
			url: '/admin/site/view/0',
			success(res) {
				datas.siteList = res.data
			}
		})
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
						url: `/admin/link/${datas.model.id}`,
						data: datas.model,
						successMsg: '修改友情链接已成功',
						beforeSend() {
							datas.loading = true
						},
						success(res) {
							proxy.$common.linkUrl('/apply/link/list') //跳转加列表页
						},
						complete() {
							datas.loading = false
						}
					})
				} else {
					proxy.$api.request({
						method: 'post',
						url: '/admin/link',
						data: datas.model,
						successMsg: '新增友情链接已成功',
						beforeSend() {
							datas.loading = true
						},
						success(res) {
							editFormRef.value.form.resetFields() //重置表单
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