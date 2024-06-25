<template>
	<div class="mainbody">
		<dt-location :data="[{title:'系统功能'},{title:'编辑平台'}]"></dt-location>
		<div class="content-box">
			<dt-form-box ref="editFormRef" v-model="datas.model" :rules="datas.rules" activeName="info">
				<el-tab-pane label="基本信息" name="info">
					<div class="tab-content">
						<el-form-item label="是否启用">
							<el-switch :active-value="0" :inactive-value="1" v-model="datas.model.status"></el-switch>
						</el-form-item>
						<el-form-item label="显示图片">
							<dt-upload-image v-model="datas.model.imgUrl"></dt-upload-image>
						</el-form-item>
						<el-form-item prop="title" label="标题名称">
							<el-input v-model="datas.model.title" placeholder="必填，128字符内"></el-input>
						</el-form-item>
						<el-form-item prop="type" label="收款类型">
							<el-radio-group v-model="datas.model.type">
								<el-radio-button label="0">线下</el-radio-button>
								<el-radio-button label="1">余额</el-radio-button>
								<el-radio-button label="2">微信</el-radio-button>
								<el-radio-button label="3">支付宝</el-radio-button>
							</el-radio-group>
						</el-form-item>
						<el-form-item prop="sortId" label="排序数字">
							<el-input v-model="datas.model.sortId" placeholder="数字越小越排前"></el-input>
						</el-form-item>
						<el-form-item label="支付接口">
							<el-input v-model="datas.model.payUrl" placeholder="可空，512字符内"></el-input>
						</el-form-item>
						<el-form-item label="通知接口">
							<el-input v-model="datas.model.notifyUrl" placeholder="可空，512字符内"></el-input>
						</el-form-item>
						<el-form-item label="备注说明">
							<el-input type="textarea" :rows="3" v-model="datas.model.remark" placeholder="可空，512字符内" maxlength="512" show-word-limit></el-input>
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
		model: {
			id: 0,
			title: null,
			imgUrl: null,
			remark: null,
			type: 0,
			payUrl: null,
			notifyUrl: null,
			sortId: 99,
			status: 0
		},
		rules: {
			title: [
				{ required: true, message: '请输入平台名称', trigger: 'blur' }
			],
			type: [
				{ required: true, message: '请选择收款类型', trigger: 'change' }
			],
			sortId: [
				{ required: true, message: '请输入排序数字', trigger: 'blur' }
			]
		}
	})
	
	//初始化数据
	const initData = async() => {
		//修改时赋值
		if (props.id) {
			proxy.$api.request({
				url: `/admin/payment/${props.id}`,
				loading: true,
				success(res) {
					//赋值给model
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
						url: `/admin/payment/${datas.model.id}`,
						data: datas.model,
						successMsg: '修改支付平台已成功',
						beforeSend() {
							datas.loading = true
						},
						success(res) {
							//可跳转加列表页
							proxy.$common.linkUrl('/setting/payee/list')
						},
						complete() {
							datas.loading = false
						}
					})
				} else {
					proxy.$api.request({
						method: 'post',
						url: '/admin/payment',
						data: datas.model,
						successMsg: '新增支付平台已成功',
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